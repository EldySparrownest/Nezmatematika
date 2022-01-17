using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMMathProblemsBase.Model
{
    public enum PublishingStatus
    {
        NotPublished = 0,
        ChangesSincePublished = 1,
        PublishedUpToDate = 2
    }
}
