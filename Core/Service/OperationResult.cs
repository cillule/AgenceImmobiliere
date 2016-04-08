using System.Collections.Generic;
using System.Runtime.Serialization;
using Oyosoft.AgenceImmobiliere.Core.Tools;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.Service
{
    [DataContract]
    public class OperationResult : BaseNotifyPropertyChanged
    {
        private bool _success;
        private ErrorsList _errors;
        private ErrorsList _warnings;

        [DataMember]
        public bool Success { get { return _success; } set { SetProperty(ref _success, value); } }
        [DataMember]
        public ErrorsList Errors { get { return _errors; } private set { SetProperty(ref _errors, value); } }
        [DataMember]
        public ErrorsList Warnings { get { return _warnings; } private set { SetProperty(ref _errors, value); } }

        public OperationResult()
        {
            this.Success = false;
            this._errors = new ErrorsList();
            this._warnings = new ErrorsList();
        }
    }

    [DataContract]
    public class StringOperationResult : OperationResult
    {
        private string _result;

        [DataMember]
        public string Result { get { return _result; } set { SetProperty(ref _result, value); } }

        public StringOperationResult() : base()
        {
            this._result = "";
        }
        public StringOperationResult(string result) : this()
        {
            this.Result = result;
        }
    }

    [DataContract]
    public class OperationResult<T> : OperationResult where T : new()
    {
        private T _result;

        [DataMember]
        public T Result { get { return _result; } set { SetProperty(ref _result, value); } }

        public OperationResult() : base()
        {
            this._result = new T();
        }
        public OperationResult(T item) : this()
        {
            this.Result = item;
        }
    }

    [DataContract]
    public class OperationResult<T1, T2> : OperationResult<T1> where T1 : new() where T2 : new()
    {
        private T2 _result2;

        [DataMember]
        public T2 Result2 { get { return _result2; } set { SetProperty(ref _result2, value); } }

        public OperationResult() : base()
        {
            this._result2 = new T2();
        }
        public OperationResult(T1 item1) : base(item1)
        {
            this._result2 = new T2();
        }
        public OperationResult(T1 item1, T2 item2) : this(item1)
        {
            this._result2 = item2;
        }
    }
}
