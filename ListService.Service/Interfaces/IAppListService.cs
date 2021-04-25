namespace ListService.Service.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts.Domain.V1;

    public interface IAppListService
    {
        Task<List<AppList>> ListPostsAsync(ListAllListsFilter filter = null, PaginationFilter paginationFilter = null);

        Task<bool> CreateListAsync(AppList list);

        Task<AppList> GetListByIdAsync(Guid listId);

        Task<bool> UpdateListAsync(AppList listToUpdate);

        Task<bool> DeleteListAsync(Guid listId);

        Task<bool> UserOwnsListAsync(Guid listId, string userId);

        Task<List<Item>> GetAllItemsAsync();

        Task<bool> CreateItemAsync(Item item);

        Task<Item> GetItemByNameAsync(string itemName);

        Task<bool> DeleteItemAsync(string itemName);
    }
}