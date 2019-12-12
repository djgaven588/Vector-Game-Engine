using System;
using System.Collections.Generic;
using System.Reflection;
using Svelto.DataStructures;
using Svelto.ECS.Internal;
using Svelto.Utilities;

namespace Svelto.ECS
{
    public class EntityBuilder<T> : IEntityBuilder where T : struct, IEntityStruct
    {
        static class EntityView
        {
            internal static readonly FasterList<KeyValuePair<Type, ActionCast<T>>> cachedFields;
            internal static readonly Dictionary<Type, Type[]>                      cachedTypes;
            internal static readonly Dictionary<Type, ECSTuple<object, int>> implementorsByType;

#pragma warning disable CA1810 // Initialize reference type static fields inline
            static EntityView()
#pragma warning restore CA1810 // Initialize reference type static fields inline
            {
                cachedFields = new FasterList<KeyValuePair<Type, ActionCast<T>>>();

                var type = typeof(T);

                var fields = type.GetFields(BindingFlags.Public |
                                            BindingFlags.Instance);

                for (var i = fields.Length - 1; i >= 0; --i)
                {
                    var field = fields[i];

                    var setter = FastInvoke<T>.MakeSetter(field);

                    cachedFields.Add(new KeyValuePair<Type, ActionCast<T>>(field.FieldType, setter));
                }

                cachedTypes = new Dictionary<Type, Type[]>();
                
                implementorsByType = new Dictionary<Type, ECSTuple<object, int>>();
            }

            internal static void InitCache()
            {}

            internal static void BuildEntityView(out T entityView)
            {
                entityView = new T();
            }
        }

        public EntityBuilder()
        {
            _initializer = DEFAULT_IT;

            EntityBuilderUtilities.CheckFields(ENTITY_VIEW_TYPE, NEEDS_REFLECTION);
        }

        public EntityBuilder(in T initializer) : this()
        {
            _initializer = initializer;
        }

        public void BuildEntityAndAddToList(ref ITypeSafeDictionary dictionary, EGID egid,
            IEnumerable<object> implementors)
        {
            if (dictionary == null)
                dictionary = new TypeSafeDictionary<T>();

            var castedDic = dictionary as TypeSafeDictionary<T>;

            DBC.ECS.Check.Require(!castedDic.ContainsKey(egid.entityID),
                    $"building an entity with already used entity id! id: '{egid.entityID}'");

            castedDic.Add(egid.entityID, _initializer);
        }

        ITypeSafeDictionary IEntityBuilder.Preallocate(ref ITypeSafeDictionary dictionary, uint size)
        {
            return Preallocate(ref dictionary, size);
        }

        static ITypeSafeDictionary Preallocate(ref ITypeSafeDictionary dictionary, uint size)
        {
            if (dictionary == null)
                dictionary = new TypeSafeDictionary<T>(size);
            else
                dictionary.SetCapacity(size);

            return dictionary;
        }

        public Type GetEntityType()
        {
            return ENTITY_VIEW_TYPE;
        }

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static EntityBuilder()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            ENTITY_VIEW_TYPE = typeof(T);
            DEFAULT_IT = default;
            HAS_EGID = typeof(INeedEGID).IsAssignableFrom(ENTITY_VIEW_TYPE);
            SetEGIDWithoutBoxing<T>.Warmup();
        }

        readonly T                        _initializer;
        internal static readonly Type ENTITY_VIEW_TYPE;
        public static readonly bool HAS_EGID;

        static readonly T      DEFAULT_IT;
        const  bool            NEEDS_REFLECTION = false;
    }
}