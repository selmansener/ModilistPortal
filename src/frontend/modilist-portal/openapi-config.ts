import type { ConfigFile } from '@rtk-query/codegen-openapi'

const config: ConfigFile = {
  schemaFile: 'https://localhost:5088/swagger/v1/swagger.json',
  apiFile: './src/store/emptyApi.ts',
  apiImport: 'emptySplitApi',
  outputFile: './src/store/api.ts',
  exportName: 'api',
  hooks: true
}

export default config