using Svelto.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Svelto.Tasks.Enumerators
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    /// <summary>
    /// /// Yield a function that control the flow execution through the return value.
    /// </summary>
    /// <typeparam name="T">
    /// facilitate the use of counters that can be passed by reference to the callback function
    /// </typeparam>
    public class SmartFunctionEnumerator<T> : IEnumerator, IEnumerator<T>
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        public SmartFunctionEnumerator(FuncRef<T, bool> func)
        {
            _func = func;
            _value = default(T);
        }

        public T field
        {
            get { return _value; }
        }

        public bool MoveNext()
        {
            return _func(ref _value);
        }

        public void Reset()
        { }

        T IEnumerator<T>.Current
        {
            get { return _value; }
        }

        public object Current
        {
            get { return null; }
        }

        public override string ToString()
        {
            if (_name == null)
            {
                var method = _func.GetMethodInfoEx();

                _name = method.GetDeclaringType().Name.FastConcat(".", method.Name);
            }

            return _name;
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        { }

        FuncRef<T, bool> _func;
        T _value;

        string _name;
    }
}