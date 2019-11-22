using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TinyLittleMvvm;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;

namespace ManagerBBCC.Main.Controls.Dialogs
{
    public class BlackListDialogViewModel : DialogViewModel<MessageDialogResult>
    {
        public BlackListDialogViewModel()
        {
            _dialogCommand = new RelayCommand<MessageDialogResult>(OnDialogCommandExecute, result => true);
        }

        private ICommand _dialogCommand;
        public ICommand DialogCommand => this._dialogCommand;

        private string _title;
        public string Title
        {
            get => this._title;
            set
            {
                if (this._title == value) return;

                this._title = value;
                this.NotifyOfPropertyChange(nameof(this.Title));
            }
        }

        private string _message;
        public string Message
        {
            get => this._message;
            set
            {
                if (this._message == value) return;

                this._message = value;
                this.NotifyOfPropertyChange(nameof(this.Message));
            }
        }

        private string _affirmativeButtonText = "Yes";
        public string AffirmativeButtonText
        {
            get => this._affirmativeButtonText;
            set
            {
                if (this._affirmativeButtonText == value) return;

                this._affirmativeButtonText = value;
                this.NotifyOfPropertyChange(nameof(this.AffirmativeButtonText));
            }
        }

        private string _negativeButtonText = "No";
        public string NegativeButtonText
        {
            get => this._negativeButtonText;
            set
            {
                if (this._negativeButtonText == value) return;

                this._negativeButtonText = value;
                this.NotifyOfPropertyChange(nameof(this.NegativeButtonText));
            }
        }

        private void OnDialogCommandExecute(MessageDialogResult messageDialogResult)
        {
            this.Close(messageDialogResult);
        }

    }
}
