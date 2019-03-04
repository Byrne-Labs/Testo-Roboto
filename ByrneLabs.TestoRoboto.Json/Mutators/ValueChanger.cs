﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ByrneLabs.TestoRoboto.Json.Mutators
{
    public abstract class ValueChanger : JsonMutator
    {
        protected abstract IEnumerable<object> TestValues { get; }

        public override IEnumerable<JObject> MutateJsonMessage(JObject message)
        {
            var mutatedMessages = new List<JObject>();
            foreach (var property in message.Descendants().OfType<JProperty>().Where(p => p.HasValues))
            {
                mutatedMessages.AddRange(ChangeValues(property));
            }

            return mutatedMessages;
        }

        private IEnumerable<JObject> ChangeValues(JProperty property)
        {
            var mutatedMessages = new List<JObject>();
            foreach (var testValue in TestValues)
            {
                var clonedMessage = (JObject) property.Root.DeepClone();
                var clonedProperty = (JProperty) clonedMessage.SelectToken(property.Path).Parent;

                switch (testValue)
                {
                    case null:
                        clonedProperty.Value = null;
                        break;
                    case string value:
                        clonedProperty.Value = value;
                        break;
                    case DateTime value:
                        clonedProperty.Value = value;
                        break;
                    case bool value:
                        clonedProperty.Value = value;
                        break;
                    case int value:
                        clonedProperty.Value = value;
                        break;
                    case long value:
                        clonedProperty.Value = value;
                        break;
                    case decimal value:
                        clonedProperty.Value = value;
                        break;
                    case float value:
                        clonedProperty.Value = value;
                        break;
                    case double value:
                        clonedProperty.Value = value;
                        break;
                    case Guid value:
                        clonedProperty.Value = value.ToString();
                        break;
                    default:
                        throw new ArgumentException("Unexpected type");
                }

                mutatedMessages.Add(clonedMessage);
            }

            return mutatedMessages;
        }
    }
}
