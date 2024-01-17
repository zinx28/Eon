const express = require("express")
const application = express()

const config = require("./config.json")
const apis = require("./apis.js")
const server = require("./server.js")

server.init(application,apis, config);

