﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public bool BackStageVisible { get; set; } = false;

        public bool RibbonTabsVisible { get; set; } = false;

        public string Something => "asdf";

        public bool StartScreenVisible { get; set; } = true;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
