using System;
using System.Runtime.Serialization;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.Tools
{
    [DataContract]
    public class Error : BaseNotifyPropertyChanged
    {
        private Enums.ErrorType _type;
        private Exception _exception;
        private string _message;


        [DataMember]
        public Enums.ErrorType Type { get { return this._type; } private set { SetProperty(ref _type, value); } }
        [DataMember]
        public Exception Exception { get { return this._exception; } private set { SetProperty(ref _exception, value); } }
        [DataMember]
        public string Message { get { return this._message; } private set { SetProperty(ref _message, value); } }

        public Error(string message, Enums.ErrorType type = Enums.ErrorType.Message, Exception exception = null)
        {
            this.Type = type;
            this.Exception = exception;
            this.Message = message;
        }
    }
}
