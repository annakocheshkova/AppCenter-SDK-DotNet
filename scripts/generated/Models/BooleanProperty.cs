// <auto-generated>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </auto-generated>

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Boolean property.
    /// </summary>
    [Newtonsoft.Json.JsonObject("boolean")]
    public partial class BooleanProperty : CustomProperty
    {
        /// <summary>
        /// Initializes a new instance of the BooleanProperty class.
        /// </summary>
        public BooleanProperty()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BooleanProperty class.
        /// </summary>
        /// <param name="value">Boolean property value.</param>
        public BooleanProperty(string name, bool value)
            : base(name)
        {
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets boolean property value.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public bool Value { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
        }
    }
}
