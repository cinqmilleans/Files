namespace Files.ViewModels.Search
{
    public interface IKindPageViewModel : ISettingSearchPageViewModel
    {
    }

    public class KindPageViewModel : SettingSearchPageViewModel, IKindPageViewModel
    {
        public override string Glyph => "\xF0E2";
        public override string Title => "Kind";

        public override bool HasValue => false;

        public KindPageViewModel(ISearchNavigatorViewModel navigator) : base(navigator) {}
    }
}
