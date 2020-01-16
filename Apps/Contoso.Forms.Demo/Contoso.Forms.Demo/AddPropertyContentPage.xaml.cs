// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AddPropertyContentPage
    {
        public event Action<Property> PropertyAdded;

        public AddPropertyContentPage()
        {
            InitializeComponent();
        }

        async void AddProperty(object sender, EventArgs e)
        {
            Property addedProperty = new Property(NameEntry?.Text, ValueEntry?.Text);
            PropertyAdded?.Invoke(addedProperty);
            await Navigation.PopModalAsync();
        }

        async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
