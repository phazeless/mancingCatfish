using System;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal
{
    /// <summary>
    /// A common class that derives from MonoBehavior so that we can provide a
    /// custom editor for BaseBehavior{TSerializer}
    /// </summary>
    public class CommonBaseBehavior : MonoBehaviour { }
}

namespace FullInspector
{

    public abstract class BaseBehavior : BaseBehavior<FullSerializerSerializer>
    {
    }
    /// <summary>
    /// Provides a better inspector and serialization experience in Unity.
    /// </summary>
    /// <remarks>
    /// We don't serialize anything in this type through Json.NET, as we recover
    /// the Json.NET serialized data via Unity serialization
    /// </remarks>
    /// <typeparam name="TSerializer">
    /// The type of serializer that the behavior should use.
    /// </typeparam>
    public abstract class BaseBehavior<TSerializer> :
        CommonBaseBehavior, ISerializedObject, ISerializationCallbackReceiver
        where TSerializer : BaseSerializer
    {
        static BaseBehavior()
        {
            BehaviorTypeToSerializerTypeMap.Register(typeof(BaseBehavior<TSerializer>), typeof(TSerializer));
        }

        /// <summary>
        /// This awake base method calls RestoreState() by default. If you
        /// override this method, it is *critically* important that this be the
        /// first call made in your Awake method. If it is not, then your
        /// component may not be deserialized when you go to access values.
        /// </summary>
        protected virtual void Awake()
        {
            fiSerializationManager.OnUnityObjectAwake(this);
        }

        /// <summary>
        /// This base method ensures that the object is fully deserialized before
        /// running actual validation code.
        /// </summary>
        protected virtual void OnValidate()
        {
            // TODO: We need to eliminate the isPlaying call.
            if (Application.isPlaying == false && ((ISerializedObject)this).IsRestored == false)
            {
                RestoreState();
            }
        }

        /// <summary>
        /// If true, when an object is cloned via an Instantiate call a fast path
        /// will get used, where the cloned object will use the same values as
        /// the original object. **This means that object references will be
        /// shared between the prefab and the instance, such as dictionaries.**
        ///
        /// **WARNING**: This feature is risky! You may get unexpected results.
        ///
        /// You should only use this feature if you find object cloning is a
        /// performance problem. Enabling this will almost certainly cause
        /// headache if it is not done carefully.
        ///
        /// This value can only be set to true while in play-mode or in a player.
        /// </summary>
        public bool SharedStatePrefabInstantiation
        {
            get
            {
                return !string.IsNullOrEmpty(_sharedStateGuid);
            }
            set
            {
                if (Application.isPlaying && value)
                {
                    _sharedStateGuid = Guid.NewGuid().ToString();
                }
                else
                {
                    _sharedStateGuid = string.Empty;
                }
            }
        }
        string ISerializedObject.SharedStateGuid
        {
            get { return _sharedStateGuid; }
        }
        [SerializeField, HideInInspector]
        private string _sharedStateGuid;

        /// <summary>
        /// Save the state of component so that it can go through Unity
        /// serialization correctly.
        /// </summary>
        [ContextMenu("Save Current State")]
        public void SaveState()
        {
            fiISerializedObjectUtility.SaveState<TSerializer>(this);
        }

        /// <summary>
        /// Restore the state of the component after it has gone through Unity
        /// serialization. If the component has already been restored, it will be
        /// reset to its last saved state.
        /// </summary>
        [ContextMenu("Restore Saved State")]
        public void RestoreState()
        {
            fiISerializedObjectUtility.RestoreState<TSerializer>(this);
        }

        /// <summary>
        /// Serializing references derived from UnityObject is tricky for a
        /// number of reasons, so we just let Unity handle it. The object can be
        /// modified in the inspector and be deleted, or it can become a prefab.
        /// Further, there is no good way to uniquely identify components and
        /// game objects that handle prefabs and instantiation well. We therefore
        /// just let Unity serialize our references for us.
        /// </summary>
        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not
        /// serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<UnityObject> _objectReferences;
        List<UnityObject> ISerializedObject.SerializedObjectReferences
        {
            get
            {
                return _objectReferences;
            }
            set
            {
                _objectReferences = value;
            }
        }

        /// <summary>
        /// The key fields that are serialized. These map to the
        /// properties/fields that Full Inspector has discovered in the behavior
        /// type that need to be serialized. This value needs to be serialized by
        /// Unity and therefore cannot be auto-implemented by a property.
        /// </summary>
        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not
        /// serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<string> _serializedStateKeys;
        List<string> ISerializedObject.SerializedStateKeys
        {
            get
            {
                return _serializedStateKeys;
            }
            set
            {
                _serializedStateKeys = value;
            }
        }

        /// <summary>
        /// The value fields that are serialized. These correspond to the key
        /// fields that Full Inspector has discovered in the behavior type that
        /// need to be serialized. This value needs to be serialized by Unity and
        /// therefore cannot be auto-implemented by a property.
        /// </summary>
        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not
        /// serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<string> _serializedStateValues;

        List<string> ISerializedObject.SerializedStateValues
        {
            get
            {
                return _serializedStateValues;
            }
            set
            {
                _serializedStateValues = value;
            }
        }

        bool ISerializedObject.IsRestored { get; set; }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            ((ISerializedObject)this).IsRestored = false;
            fiSerializationManager.OnUnityObjectDeserialize<TSerializer>(this);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            fiSerializationManager.OnUnityObjectSerialize<TSerializer>(this);
        }
    }
}