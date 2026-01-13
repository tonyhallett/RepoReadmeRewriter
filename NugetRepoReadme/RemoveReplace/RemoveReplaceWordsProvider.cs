using System.Collections.Generic;
using Microsoft.Build.Framework;
using NugetRepoReadme.MSBuild;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace NugetRepoReadme.RemoveReplace
{
    internal class RemoveReplaceWordsProvider : IRemoveReplaceWordsProvider
    {
        private readonly IIOHelper _ioHelper;
        private readonly IMessageProvider _messageProvider;

        public RemoveReplaceWordsProvider(IIOHelper ioHelper, IMessageProvider messageProvider)
        {
            _ioHelper = ioHelper;
            _messageProvider = messageProvider;
        }

        public List<RemoveReplaceWord> Provide(ITaskItem[]? removeReplaceWordsItems, IAddError addError)
        {
            var removeReplaceWords = new List<RemoveReplaceWord>();
            if (removeReplaceWordsItems == null)
            {
                return removeReplaceWords;
            }

            foreach (ITaskItem removeReplaceWordsItem in removeReplaceWordsItems)
            {
                string filePath = removeReplaceWordsItem.ItemSpec;
                if (!_ioHelper.FileExists(filePath))
                {
                    string errorMessage = _messageProvider.RemoveReplaceWordsFileDoesNotExist(filePath);
                    addError.AddError(errorMessage);
                }

                string[] lines = _ioHelper.ReadAllLines(filePath);
                removeReplaceWords.AddRange(RemoveReplaceWordsParser.Parse(lines));
            }

            return removeReplaceWords;
        }
    }
}
