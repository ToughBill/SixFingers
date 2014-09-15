﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WorkstationController.VisualElement;
using WorkstationController.Core.Data;

namespace WorkstationController.WorktableVisualTest
{
    /// <summary>
    /// MainWindow.xaml logic
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Worktable worktable = new Worktable(
                new Size(4000, 8000),
                new Size(20, 40), 
                new Size(20, 60),
                new Size(20, 60), new Point(50, 50), 1500, 3500, 20);
            Configurations.Instance.Worktable = worktable;
            WorktableVisual worktableVisual = new WorktableVisual();
            grid1.AttachWorktableVisual(worktableVisual);
            grid1.InvalidateVisual();
        }
    }
}
