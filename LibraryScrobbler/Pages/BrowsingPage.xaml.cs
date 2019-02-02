﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryScrobbler
{
    /// <summary>
    /// Interaction logic for BrowsingPage.xaml
    /// </summary>
    public partial class BrowsingPage : Page
    {

        private List<TreeViewItem> _items;
        public IEnumerable<TreeViewItem> Items
        {
            get { return _items.AsEnumerable(); }
            set { }
        }

        public BrowsingPage()
        {
            InitializeComponent();
        }
    }
}