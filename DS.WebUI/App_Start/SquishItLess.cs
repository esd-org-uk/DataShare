[assembly: WebActivator.PreApplicationStartMethod(typeof(DS.WebUI.App_Start.SquishItLess), "Start")]

namespace DS.WebUI.App_Start
{
    using SquishIt.Framework;
    using SquishIt.Less;

    public class SquishItLess
    {
        public static void Start()
        {
            Bundle.RegisterStylePreprocessor(new LessPreprocessor());
        }
    }
}