/// <binding ProjectOpened='Run - Development' />

module.exports = function(env) {
  return require(`./Client/webpack.${env}.js`)
}