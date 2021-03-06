﻿using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Daneel.ViewControllers
{
    [ModalCardPresentation]
    public partial class SelectBeginningOfWeekViewController
        : MvxViewController<SelectBeginningOfWeekViewModel>
    {
        public SelectBeginningOfWeekViewController()
            : base(nameof(SelectBeginningOfWeekViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new BeginningOfWeekTableViewSource(DaysTableView);
            DaysTableView.Source = source;

            var bindingSet = this.CreateBindingSet<SelectBeginningOfWeekViewController, SelectBeginningOfWeekViewModel>();

            bindingSet.Bind(source).To(vm => vm.BeginningOfWeekCollection);

            bindingSet.Bind(BackButton).To(vm => vm.CloseCommand);

            bindingSet.Bind(source)
                      .For(v => v.SelectionChangedCommand)
                      .To(v => v.SelectBeginningOfWeekCommand);

            bindingSet.Apply();

        }
    }
}

