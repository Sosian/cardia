using System;

namespace MGT.Utilities.Network
{
  [Serializable]
  public class RelayMessage
  {
    private int clientId;

    public int ClientId
    {
      get => this.clientId;
      set => this.clientId = value;
    }
  }
}