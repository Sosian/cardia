using MGT.Utilities.EventHandlers;
using System;
using System.Runtime.Serialization;

namespace MGT.Utilities.Network
{
  public abstract class NetworkRelay<T> where T : RelayMessage, new()
  {
    protected bool running;
    protected int port = 10308;
    protected int timeout = 20000;
    protected string version = "0.0.1";
    protected string nickname = nameof (Nickname);
    protected int selfId;

    public bool Running => this.running;

    public event GenericEventHandler<int> PortChanged;

    public int Port
    {
      get => this.port;
      set
      {
        if (this.running)
          throw new Exception("Cannot set port when network provider is running");
        int port = this.port;
        this.port = value;
        if (port == value || this.PortChanged == null)
          return;
        this.PortChanged((object) this, value);
      }
    }

    public event GenericEventHandler<int> TimeoutChanged;

    public int Timeout
    {
      get => this.timeout;
      set
      {
        if (this.running)
          throw new Exception("Cannot set timeout when network provider is running");
        int timeout = this.timeout;
        this.timeout = value;
        if (timeout == value || this.TimeoutChanged == null)
          return;
        this.TimeoutChanged((object) this, value);
      }
    }

    public event GenericEventHandler<string> VersionChanged;

    public string Version
    {
      get => this.version;
      set
      {
        if (this.running)
          throw new Exception("Cannot set versione when network provider is running");
        string version = this.version;
        this.version = value;
        if (!(version != value) || this.VersionChanged == null)
          return;
        this.VersionChanged((object) this, value);
      }
    }

    public event GenericEventHandler<string> NicknameChanged;

    public string Nickname
    {
      get => this.nickname;
      set
      {
        if (this.running)
          throw new Exception("Cannot set nickname when network provider is running");
        string nickname = this.nickname;
        this.nickname = value;
        if (!(nickname != value) || this.NicknameChanged == null)
          return;
        this.NicknameChanged((object) this, value);
      }
    }

    public event GenericEventHandler<int, string> OnClientConnected;

    public event GenericEventHandler<int> OnclientDisconnected;

    public event GenericEventHandler<T> OnClientMessageReceived;

    public event GenericEventHandler<string, bool> OnProviderDisconnected;

    protected virtual void ClientConnected(int clientId, string nickname)
    {
      if (!this.running)
        return;
      GenericEventHandler<int, string> onClientConnected = this.OnClientConnected;
      if (onClientConnected == null)
        return;
      onClientConnected((object) this, clientId, nickname);
    }

    protected virtual void ClientDisconnected(int clientId)
    {
      if (!this.running)
        return;
      GenericEventHandler<int> onclientDisconnected = this.OnclientDisconnected;
      if (onclientDisconnected == null)
        return;
      onclientDisconnected((object) this, clientId);
    }

    protected virtual void ClientMessageReceived(T clientMessage)
    {
      if (!this.running)
        return;
      GenericEventHandler<T> clientMessageReceived = this.OnClientMessageReceived;
      if (clientMessageReceived == null)
        return;
      clientMessageReceived((object) this, clientMessage);
    }

    protected virtual void ProviderDisconnected(string cause, bool error)
    {
      GenericEventHandler<string, bool> providerDisconnected = this.OnProviderDisconnected;
      if (providerDisconnected == null)
        return;
      providerDisconnected((object) this, cause, error);
    }

    public abstract bool Start();

    public abstract void Stop();

    public abstract void Send(T heartRateMessage);

    public void ResetSubscriptions()
    {
      this.PortChanged = (GenericEventHandler<int>) null;
      this.TimeoutChanged = (GenericEventHandler<int>) null;
      this.VersionChanged = (GenericEventHandler<string>) null;
      this.NicknameChanged = (GenericEventHandler<string>) null;
      this.OnClientConnected = (GenericEventHandler<int, string>) null;
      this.OnclientDisconnected = (GenericEventHandler<int>) null;
      this.OnClientMessageReceived = (GenericEventHandler<T>) null;
      this.OnProviderDisconnected = (GenericEventHandler<string, bool>) null;
    }

    public virtual void Init()
    {
      if (this.PortChanged != null)
        this.PortChanged((object) this, this.port);
      if (this.TimeoutChanged != null)
        this.TimeoutChanged((object) this, this.timeout);
      if (this.VersionChanged != null)
        this.VersionChanged((object) this, this.version);
      if (this.NicknameChanged == null)
        return;
      this.NicknameChanged((object) this, this.nickname);
    }
  }
}
