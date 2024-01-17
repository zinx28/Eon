const path = require("path")
const fs = require("fs")
// this was coded so quickly when i was away for a bit
module.exports.init = function init(application, config) {

  //love is not an answrr
  application.get("/launcher/info", (req, res) => {
    res.json({
      "updater": `${config.api}:${config.port}/files/${config.updaterversion}/EonUpdater.exe`,
      "launcher": `${config.api}:${config.port}/files/${config.launcherversion}/Eon.zip`,
      "launcherversion": config.launcherversion,
      "updaterversion": config.updaterversion
    })
  })

  // dev
  application.get("/launcher/info/dever", (req, res) => {
    res.json({
      "updater": `${config.api}:${config.port}/files/${config.devupdater}/EronUpdater.exe`,
      "launcher": `${config.api}:${config.port}/files/${config.devversion}/Eron.zip`,
      "launcherversion": config.devversion,
      "updaterversion": config.devupdater
    })
  })


  application.get("/files/:versionId/:filename", (req, res) => {
    const filePath = path.join(__dirname, `resources`, req.params.versionId, req.params.filename);
    fs.readFile(filePath, (err, data) => {
      if (err) {
        console.error('Not real:', err);
        return res.status(500).send('Error reading the file');
      }

      res.setHeader('Content-Type', 'application/octet-stream');
      res.setHeader('Content-Disposition', `attachment; filename="${req.params.filename}"`);
      res.send(data);
    });
  })


  application.get("/files/dxwebsetup.exe", (req, res) => {
    const filePath = path.join(__dirname, `resources`, 'dxwebsetup.exe');

    fs.readFile(filePath, (err, data) => {
      if (err) {
        console.error('Error reading the file:', err);
        return res.status(500).send('Error reading the file');
      }

      res.setHeader('Content-Type', 'application/octet-stream');
      res.setHeader('Content-Disposition', `attachment; filename="dxwebsetup.exe"`);
      res.send(data);
    });
  })


  application.get("/launcher/news", (req, res) => {
    res.json(config.news)
  })
}
