using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.Tools
{
    //[DataContract]
    public class ErrorsList : ObservableCollection<Error>
    {
        public bool IsEmpty { get { return base.Count == 0; } }

        public void Add(string message, Enums.ErrorType type = Enums.ErrorType.Message, Exception exception = null)
        {
            base.Add(new Error(message, type, exception));
        }

        public void AddRange(IList<Error> errors)
        {
            foreach (Error err in errors)
                base.Add(err);
        }
        public void AddRange(IList<Error> errors, string propertyName, BaseNotifyPropertyChanged sender)
        {
            this.AddRange(errors);
            sender.OnPropertyChanged(propertyName);
        }

        public override string ToString()
        {
            string messages = "";
            foreach (Error err in this)
            {
                if (string.IsNullOrEmpty(err.Message)) continue;
                if (messages != "") messages += "\n";
                messages += err.Message;
            }
            return messages;
        }

    }
}
