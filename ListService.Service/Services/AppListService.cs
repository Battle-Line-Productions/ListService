namespace ListService.Service.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.Domain.V1;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class AppListService : IAppListService
    {
        private readonly ApplicationDbContext _context;

        public AppListService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AppList>> ListPostsAsync(ListAllListsFilter filter = null, PaginationFilter paginationFilter = null)
        {
            var queryable = _context.AppLists.AsQueryable();

            if (paginationFilter == null)
            {
                return await queryable.ToListAsync();
            }

            queryable = AddFiltersOnQuery(filter, queryable);

            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
            return await queryable.Include(x => x.ListItems)
                .Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
        }

        public async Task<bool> CreateListAsync(AppList list)
        {
            list.ListItems?.ForEach(x => x.ItemName = x.ItemName.ToLower());

            //await AddNewTags(list);
            await _context.AppLists.AddAsync(list);

            var created = await _context.SaveChangesAsync();
            return created > 0;
        }

        public async Task<AppList> GetListByIdAsync(Guid listId)
        {
            return await _context.AppLists
                .Include(x => x.ListItems)
                .SingleOrDefaultAsync(x => x.Id == listId);
        }

        public async Task<bool> UpdateListAsync(AppList listToUpdate)
        {
            listToUpdate.ListItems?.ForEach(x => x.ItemName = x.ItemName.ToLower());
            //await AddNewTags(postToUpdate);

            _context.AppLists.Update(listToUpdate);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteListAsync(Guid listId)
        {
            var post = await GetListByIdAsync(listId);

            if (post == null)
                return false;

            _context.AppLists.Remove(post);

            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<bool> UserOwnsListAsync(Guid listId, string userId)
        {
            var list = await _context.AppLists.AsNoTracking().SingleOrDefaultAsync(x => x.Id == listId);

            if (list == null)
            {
                return false;
            }

            return list.UserId == userId;
        }

        public Task<List<Item>> GetAllItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateItemAsync(Item item)
        {
            throw new NotImplementedException();
        }

        public Task<Item> GetItemByNameAsync(string itemName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string itemName)
        {
            throw new NotImplementedException();
        }

        private async Task AddNewItems(AppList list)
        {
            foreach (var item in list.ListItems)
            {
                var existingItem = await _context.AppLists.SingleOrDefaultAsync(x => x.Name == item.ItemName);

                if (existingItem != null)
                {
                    continue;
                }

                await _context.Items.AddAsync(new Item()
                {
                    CreateDateTime = DateTime.UtcNow, CreatorId = list.UserId, Id = Guid.NewGuid(), Description = item.Item.Description, Order = item.Item.
                        Order, Title = item.Item.Title
                });
            }
        }

        private static IQueryable<AppList> AddFiltersOnQuery(ListAllListsFilter filter, IQueryable<AppList> queryable)
        {
            if (!string.IsNullOrEmpty(filter?.UserId))
            {
                queryable = queryable.Where(x => x.UserId == filter.UserId);
            }

            return queryable;
        }
    }
}