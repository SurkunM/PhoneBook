const { defineConfig } = require("@vue/cli-service")
const path = require("path");

module.exports = defineConfig({
    outputDir: path.resolve(__dirname, "..", "wwwroot"),

    devServer: {
        proxy: {
            "^/api": {
                target: "https://localhost:44321"
            }
        }
    },

    transpileDependencies: true,

    pluginOptions: {
        vuetify: {
            // https://github.com/vuetifyjs/vuetify-loader/tree/next/packages/vuetify-loader
        }
    }
})