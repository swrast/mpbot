namespace bot.Commands
{
    public class LinusCommand : CommandBase
    {
        [HandleMessage("линус")]
        public async Task Linus()
        {
            string? pic = (await _api.GetPictures()).ElementAtOrDefault(0);
            if (pic == null)
            {
                throw new Exception("нужно прикрепить картинку");
            }
        }
    }
}