using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;

namespace Mobile.Solution.Infrastructure.Requests.NSI
{
    public static class NsiManager
    {
        public static async Task<List<T>> GetNsi<T>(string uniqueId, string limit = null)
            where T : class, INsiItem
        {
            var item = Activator.CreateInstance(typeof(T)) as T;
            var nsi = new NsiRequest<T>();
            return await nsi.InitRequest(
                $"uniqueId={uniqueId}/{item?.RequestParameter}/{(string.IsNullOrEmpty(limit) ? "null" : limit)}");
        }

        /// <summary>
        ///     Функция поиска НСИ по значениям.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit"></param>
        /// <param name="searchProperties">Поля, по которым производится поиск.</param>
        /// <exception cref="ArgumentException">Пустая коллекция.</exception>
        /// <returns></returns>
        public static async Task<List<T>> SearchNsi<T>(string uniqueId, string limit,
            params Tuple<string, string>[] searchProperties)
            where T : class, INsiItem
        {
            if (!searchProperties.Any())
                throw new ArgumentException("Количество свойств должно быть больше нуля.");

            var item = Activator.CreateInstance(typeof(T)) as T;
            var nsi = new NsiRequest<T>();
            var parameters = string.Empty;
            foreach (var sp in searchProperties)
                parameters += $"searchProperties={sp.Item1}({sp.Item2})$";

            return await
                nsi.InitRequest(
                    $"Search/uniqueId={uniqueId}/{item?.RequestParameter}/{(string.IsNullOrEmpty(limit) ? "null" : limit)}?{parameters}");
        }

        /// <summary>
        ///     Функция поиска первого совпадения НСИ по значениям.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit"></param>
        /// <param name="searchProperties">Поля, по которым производится поиск.</param>
        /// <exception cref="ArgumentException">Пустая коллекция.</exception>
        /// <returns></returns>
        public static Task<List<T>> MatchNsi<T>(string uniqueId, string limit,
            params Tuple<string, string>[] searchProperties)
            where T : class, INsiItem
        {
            if (!searchProperties.Any())
                throw new ArgumentException("Количество свойств должно быть больше нуля.");

            var item = Activator.CreateInstance(typeof(T)) as T;
            var nsi = new NsiRequest<T>();
            var parameters = string.Empty;
            foreach (var sp in searchProperties)
                parameters += $"searchProperties={sp.Item1}({sp.Item2})$";

            return
                nsi.InitRequest(
                    $"Match/uniqueId={uniqueId}/{item?.RequestParameter}/{(string.IsNullOrEmpty(limit) ? "null" : limit)}?{parameters}");
        }
    }
}