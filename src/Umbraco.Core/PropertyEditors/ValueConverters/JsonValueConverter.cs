﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.DI;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.Core.PropertyEditors.ValueConverters
{
    /// <summary>
    /// The default converter for all property editors that expose a JSON value type
    /// </summary>
    /// <remarks>
    /// Since this is a default (umbraco) converter it will be ignored if another converter found conflicts with this one.
    /// </remarks>
    [DefaultPropertyValueConverter]
    public class JsonValueConverter : PropertyValueConverterBase
    {
        /// <summary>
        /// It is a converter for any value type that is "JSON"
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            var propertyEditor = Current.PropertyEditors[propertyType.PropertyEditorAlias];
            if (propertyEditor == null) return false;
            return propertyEditor.ValueEditor.ValueType.InvariantEquals(PropertyEditorValueTypes.Json);
        }

        public override Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            return typeof (JToken);
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType)
        {
            return PropertyCacheLevel.Content;
        }

        public override object ConvertSourceToInter(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;
            var sourceString = source.ToString();

            if (sourceString.DetectIsJson())
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject(sourceString);
                    return obj;
                }
                catch (Exception ex)
                {
                    Current.Logger.Error<JsonValueConverter>("Could not parse the string " + sourceString + " to a json object", ex);                    
                }
            }
            
            //it's not json, just return the string
            return sourceString;
        }

        //TODO: Now to convert that to XPath!
    }
}