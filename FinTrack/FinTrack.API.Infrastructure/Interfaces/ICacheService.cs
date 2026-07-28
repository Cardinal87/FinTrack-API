namespace FinTrack.API.Infrastructure.Interfaces
{
    public interface ICacheService
    {
        /// <summary>
        /// Method for saving <paramref name="value"/> to cache
        /// </summary>
        /// <typeparam name="T">type of object to save</typeparam>
        /// <param name="key">key what be used to find value in cache</param>
        /// <param name="value">value of <typeparamref name="T"/> object</param>
        /// <param name="ttl">value storage duration</param>
        /// <param name="ct">cancellation token</param>
        /// <returns><see cref="Task"/></returns>
        Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T: class;

        /// <summary>
        /// Method for retrieving object by <paramref name="key"/>
        /// </summary>
        /// <typeparam name="T">type of the resulting object</typeparam>
        /// <param name="key">key what be used to find value in cache</param>
        /// <param name="ct">cancellation token</param>
        /// <returns><see cref="Task"/> containing object of <typeparamref name="T"/> type</returns>
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T: class;

        /// <summary>
        /// Method for removal object from cache by <paramref name="key"/>
        /// </summary>
        /// <param name="key">key what be used to find value in cache</param>
        /// <param name="ct">cancellation token</param>
        /// <returns><see cref="Task"/></returns>
        Task RemoveByKeyAsync(string key, CancellationToken ct = default);

        /// <summary>
        /// Method for batch removal object from cache by <paramref name="pattern"/>  
        /// </summary>
        /// <param name="pattern">pattern what be used to find group of values in cache</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);

        /// <summary>
        /// Sets only key with empty value if key not exists yet
        /// </summary>
        /// <param name="key">key what be used to find value in cache</param>
        /// <param name="ttl">key storage duration</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>operation status as <see cref="bool"/></returns>
        Task<bool> TrySetKeyOnlyAsync(string key, TimeSpan ttl, CancellationToken ct = default);
    }
}