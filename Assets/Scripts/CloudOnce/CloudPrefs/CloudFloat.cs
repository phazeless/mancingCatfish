// <copyright file="CloudFloat.cs" company="Jan Ivar Z. Carlsen, Sindri J�elsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri J�elsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce.CloudPrefs
{
    using Internal;

    /// <summary>
    /// Used to store <see cref="float"/>s in the cloud.
    /// </summary>
    public sealed class CloudFloat : PersistentValue<float>
    {
        /// <summary>
        /// Used to store <see cref="float"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) value.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The starting value for this <see cref="float"/>.</param>
        public CloudFloat(string key, PersistenceType persistenceType, float value = 0f)
            : base(key, persistenceType, value, value, DataManager.GetFloat, DataManager.SetFloat)
        {
        }

        /// <summary>
        /// Used to store <see cref="float"/>s in the cloud.
        /// </summary>
        /// <param name="key">A unique identifier used to identify this particular value.</param>
        /// <param name="persistenceType">
        /// The method of conflict resolution to be used in case of a data conflict. Can happen if the data is altered by a different device.
        /// <see cref="PersistenceType.Latest"/> will prefer the latest (newest) value.
        /// <see cref="PersistenceType.Highest"/> will prefer the highest value.
        /// <see cref="PersistenceType.Lowest"/> will prefer the lowest value.
        /// </param>
        /// <param name="value">The starting value for this <see cref="float"/>.</param>
        /// <param name="defaultValue">The value the <see cref="float"/> will be set to if it is ever reset.</param>
        public CloudFloat(string key, PersistenceType persistenceType, float value, float defaultValue)
            : base(key, persistenceType, value, defaultValue, DataManager.GetFloat, DataManager.SetFloat)
        {
        }
    }
}