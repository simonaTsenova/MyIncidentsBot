using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;
using System.Linq;

namespace MyIncidentsBot.Helpers
{
    public class FormCustomizer
    {
        public static IFormBuilder<T> CreateCustomForm<T>()
            where T : class
        {
            var form = new FormBuilder<T>();

            var yesTerms = form.Configuration.Yes.ToList();
            var customYesTerms = new List<string>() { "Yup", "Yeah", "Sure", "Of course", "Ok", "I do", "Yes, I do" };
            yesTerms.AddRange(customYesTerms);
            form.Configuration.Yes = yesTerms.ToArray();

            var noTerms = form.Configuration.No.ToList();
            var customNoTerms = new List<string>() { "Nope", "I don't", "I don't anymore", "Not anymore", "Not really", "I changed my mind" };
            noTerms.AddRange(customNoTerms);
            form.Configuration.No = noTerms.ToArray();

            var command = form.Configuration.Commands[FormCommand.Quit];
            var quitTerms = command.Terms.ToList();
            var customQuitTerms = new List<string>() { "Cancel", "I don't want to create incident anymore", "I don't want to create ticket anymore", "I changed my mind" };
            quitTerms.AddRange(customQuitTerms);
            command.Terms = quitTerms.ToArray();

            return form;
        }
    }
}