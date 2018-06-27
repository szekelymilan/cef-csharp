using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace cef_csharp {
  public partial class CEF : Form {
    public ChromiumWebBrowser chromeBrowser;
    public string currAddress;

    public CEF() {
      InitializeComponent();
      InitializeChromium();
    }

    private void CEF_Load(object sender, EventArgs e) {

    }

    public void InitializeChromium() {
      Cef.Initialize(new CefSettings());
      chromeBrowser = new ChromiumWebBrowser("https://szekelymilan.eu");
      this.Controls.Add(chromeBrowser);
      chromeBrowser.Dock = DockStyle.Fill;
      chromeBrowser.LifeSpanHandler = new BrowserLifeSpanHandler();

      currAddress = "";
      chromeBrowser.TitleChanged += OnBrowserTitleChanged;
      chromeBrowser.AddressChanged += OnBrowserAddressChanged;
    }

    private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs e) {
      Uri myUri = new Uri(e.Address);
      currAddress = myUri.Host;
    }

    private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args) {
      this.Invoke(new Action(() => this.Text = args.Title));
      var client = new System.Net.WebClient();

      Directory.CreateDirectory(Path.GetTempPath() + "cef-csharp");
      var imageName = Path.GetTempPath() + "cef-csharp/" + generateRandomName();
      client.DownloadFile("https://www.google.com/s2/favicons?domain=" + currAddress, imageName);
      this.Invoke(new Action(() => this.Icon = Icon.FromHandle(((Bitmap)Image.FromFile(imageName)).GetHicon())));
    }

    private Random random = new Random();
    public string generateRandomName() {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      return new string(Enumerable.Repeat(chars, 8)
        .Select(s => s[random.Next(s.Length)]).ToArray()) + ".ico";
    }

    private void CEF_FormClosing(object sender, FormClosingEventArgs e) {
      Cef.Shutdown();
      Application.Exit();
    }
  }

  public class BrowserLifeSpanHandler : ILifeSpanHandler {
    public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser) {
      browserControl.Load(targetUrl);
      newBrowser = null;
      return true;
    }

    public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser) {

    }

    public bool DoClose(IWebBrowser browserControl, IBrowser browser) {
      return false;
    }

    public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser) {

    }
  }
}