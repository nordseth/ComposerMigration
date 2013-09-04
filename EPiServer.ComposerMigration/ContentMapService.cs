using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
    public class ContentMapService
    {
        private static readonly ConcurrentDictionary<object, ContentMap> _contentMaps = new ConcurrentDictionary<object, ContentMap>();

        /// <summary>
        /// Retrieves a ComposerContentMap for the provided key. If none exist a new instance is created.
        /// </summary>
        /// <param name="key">Context object used as key.</param>
        /// <returns>A ComposerContentMap instance.</returns>
        public ContentMap Get(object key)
        {
            return _contentMaps.GetOrAdd(key, new ContentMap());
        }

        public void Remove(object key)
        {
            ContentMap map;
            _contentMaps.TryRemove(key, out map);
        }

    }
}
