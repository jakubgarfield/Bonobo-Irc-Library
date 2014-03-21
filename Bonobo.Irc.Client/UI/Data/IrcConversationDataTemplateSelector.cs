using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bonobo.Irc.Client.UI.Data
{
    public class IrcConversationDataTemplateSelector : DataTemplateSelector
    {
        private readonly IDictionary<Type, DataTemplate> _templateDictionary = new Dictionary<Type, DataTemplate>()
        {
            { typeof(UIServerConversation), new IrcServerConversationTemplate().DataTemplate },
            { typeof(UIChannelConversation), new IrcChannelConversationTemplate().DataTemplate },
            { typeof(UIPersonConversation), new IrcPersonConversationTemplate().DataTemplate },
        };

        public IrcConversationDataTemplateSelector()
        {
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var dataTemplate = new DataTemplate();
            if (_templateDictionary.TryGetValue(item.GetType(), out dataTemplate))
            {
                return dataTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}
