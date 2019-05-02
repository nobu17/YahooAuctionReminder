using System;
using System.ComponentModel;

namespace YahooAuctionRemainder.Common
{
    /// <summary>
    /// データバインディング基底クラス
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
