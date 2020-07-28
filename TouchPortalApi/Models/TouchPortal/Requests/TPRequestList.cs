using System.Collections.Generic;

namespace TouchPortalApi.Models.TouchPortal.Requests {
  internal class TPRequestList : TPRequestBase {
    public List<string> Value { get; set; }
  }
}
