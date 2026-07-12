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
        public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T: class;

        /// <summary>
        /// Method for retrieving object by <paramref name="key"/>
        /// </summary>
        /// <typeparam name="T">type of the resulting object</typeparam>
        /// <param name="key">key what be used to find value in cache</param>
        /// <param name="ct">cancellation token</param>
        /// <returns><see cref="Task"/> containing object of <typeparamref name="T"/> type</returns>
        public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T: class;

        /// <summary>
        /// Method for removal object from cache by <paramref name="key"/>
        /// </summary>
        /// <param name="key">key what be used to find value in cache</param>
        /// <param name="ct">cancellation token</param>
        /// <returns><see cref="Task"/></returns>
        public Task RemoveByKeyAsync(string key, CancellationToken ct = default);

        /// <summary>
        /// Method for batch removal object from cache by <paramref name="pattern"/>  
        /// </summary>
        /// <param name="pattern">pattern what be used to find group of values in cache</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        public Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);
    }
}