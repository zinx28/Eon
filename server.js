
module.exports.init = function server(application, apis, config) {
  application.listen(config.port, () => {
    console.log(`Servers up on port ${config.port} `)
    apis.init(application, config);
  })
  
  application.get("/ping", (req, res) => {
    res.status(201)
    res.json({ status: "OK" })
  })
}
