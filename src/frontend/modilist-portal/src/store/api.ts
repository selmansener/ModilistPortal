import { emptySplitApi as api } from "./emptyApi";
const injectedRtkApi = api.injectEndpoints({
  endpoints: (build) => ({
    getApiV1AccountGet: build.query<
      GetApiV1AccountGetApiResponse,
      GetApiV1AccountGetApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Account/Get`,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    getApiV1AccountVerifyByEmail: build.query<
      GetApiV1AccountVerifyByEmailApiResponse,
      GetApiV1AccountVerifyByEmailApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Account/Verify/${queryArg.email}`,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    postApiV1AccountCreate: build.mutation<
      PostApiV1AccountCreateApiResponse,
      PostApiV1AccountCreateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Account/Create`,
        method: "POST",
        body: queryArg.createAccount,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    postApiV1AccountActivate: build.mutation<
      PostApiV1AccountActivateApiResponse,
      PostApiV1AccountActivateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Account/Activate`,
        method: "POST",
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    getApiV1AddressGetCities: build.query<
      GetApiV1AddressGetCitiesApiResponse,
      GetApiV1AddressGetCitiesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Address/GetCities`,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    getApiV1AddressGetDistrictsByCityCode: build.query<
      GetApiV1AddressGetDistrictsByCityCodeApiResponse,
      GetApiV1AddressGetDistrictsByCityCodeApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Address/GetDistricts/${queryArg.cityCode}`,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    postDevV1Seed: build.mutation<
      PostDevV1SeedApiResponse,
      PostDevV1SeedApiArg
    >({
      query: (queryArg) => ({
        url: `/dev/v1/Seed`,
        method: "POST",
        headers: { "X-ApiKey": queryArg["X-ApiKey"] },
        params: {
          seedServiceType: queryArg.seedServiceType,
          recreateDb: queryArg.recreateDb,
        },
      }),
    }),
    getDevV1GetClientIp: build.query<
      GetDevV1GetClientIpApiResponse,
      GetDevV1GetClientIpApiArg
    >({
      query: (queryArg) => ({
        url: `/dev/v1/GetClientIP`,
        headers: { "X-ApiKey": queryArg["X-ApiKey"] },
      }),
    }),
    postApiV1ProductUploadProductExcel: build.mutation<
      PostApiV1ProductUploadProductExcelApiResponse,
      PostApiV1ProductUploadProductExcelApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Product/UploadProductExcel`,
        method: "POST",
        body: queryArg.body,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    getApiV1ProductQueryUploadHistory: build.query<
      GetApiV1ProductQueryUploadHistoryApiResponse,
      GetApiV1ProductQueryUploadHistoryApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Product/QueryUploadHistory`,
        params: { "api-version": queryArg["api-version"], dqb: queryArg.dqb },
      }),
    }),
    getApiV1ProductQueryUploadHistoryDetailsByProductExcelUploadId: build.query<
      GetApiV1ProductQueryUploadHistoryDetailsByProductExcelUploadIdApiResponse,
      GetApiV1ProductQueryUploadHistoryDetailsByProductExcelUploadIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Product/QueryUploadHistoryDetails/${queryArg.productExcelUploadId}`,
        params: { "api-version": queryArg["api-version"], dqb: queryArg.dqb },
      }),
    }),
    getApiV1ProductQuery: build.query<
      GetApiV1ProductQueryApiResponse,
      GetApiV1ProductQueryApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Product/Query`,
        params: { "api-version": queryArg["api-version"], dqb: queryArg.dqb },
      }),
    }),
    getApiV1ProductGetByProductId: build.query<
      GetApiV1ProductGetByProductIdApiResponse,
      GetApiV1ProductGetByProductIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Product/Get/${queryArg.productId}`,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    postApiV1ProductByProductIdAddImages: build.mutation<
      PostApiV1ProductByProductIdAddImagesApiResponse,
      PostApiV1ProductByProductIdAddImagesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Product/${queryArg.productId}/AddImages`,
        method: "POST",
        body: queryArg.body,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    getApiV1TenantGet: build.query<
      GetApiV1TenantGetApiResponse,
      GetApiV1TenantGetApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Tenant/Get`,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
    postApiV1TenantUpsert: build.mutation<
      PostApiV1TenantUpsertApiResponse,
      PostApiV1TenantUpsertApiArg
    >({
      query: (queryArg) => ({
        url: `/api/v1/Tenant/Upsert`,
        method: "POST",
        body: queryArg.upsertTenant,
        params: { "api-version": queryArg["api-version"] },
      }),
    }),
  }),
  overrideExisting: false,
});
export { injectedRtkApi as api };
export type GetApiV1AccountGetApiResponse =
  /** status 200 Success */ AccountDtoResponseModel;
export type GetApiV1AccountGetApiArg = {
  "api-version"?: string;
};
export type GetApiV1AccountVerifyByEmailApiResponse = unknown;
export type GetApiV1AccountVerifyByEmailApiArg = {
  email: string;
  "api-version"?: string;
};
export type PostApiV1AccountCreateApiResponse =
  /** status 200 Success */ AccountDtoResponseModel;
export type PostApiV1AccountCreateApiArg = {
  "api-version"?: string;
  createAccount: CreateAccount;
};
export type PostApiV1AccountActivateApiResponse =
  /** status 200 Success */ AccountDtoResponseModel;
export type PostApiV1AccountActivateApiArg = {
  "api-version"?: string;
};
export type GetApiV1AddressGetCitiesApiResponse =
  /** status 200 Success */ City[];
export type GetApiV1AddressGetCitiesApiArg = {
  "api-version"?: string;
};
export type GetApiV1AddressGetDistrictsByCityCodeApiResponse =
  /** status 200 Success */ District[];
export type GetApiV1AddressGetDistrictsByCityCodeApiArg = {
  cityCode: string;
  "api-version"?: string;
};
export type PostDevV1SeedApiResponse = unknown;
export type PostDevV1SeedApiArg = {
  seedServiceType?: SeedServiceType;
  recreateDb?: boolean;
  /** X-ApiKey */
  "X-ApiKey": string;
};
export type GetDevV1GetClientIpApiResponse = unknown;
export type GetDevV1GetClientIpApiArg = {
  /** X-ApiKey */
  "X-ApiKey": string;
};
export type PostApiV1ProductUploadProductExcelApiResponse = unknown;
export type PostApiV1ProductUploadProductExcelApiArg = {
  "api-version"?: string;
  body: {
    file?: Blob;
  };
};
export type GetApiV1ProductQueryUploadHistoryApiResponse =
  /** status 200 Success */ ProductExcelUploadDtodqbResultDtoResponseModel;
export type GetApiV1ProductQueryUploadHistoryApiArg = {
  "api-version"?: string;
  /** DynamicQuery */
  dqb?: string;
};
export type GetApiV1ProductQueryUploadHistoryDetailsByProductExcelUploadIdApiResponse =
  /** status 200 Success */ QueryProductExcelRowDtodqbResultDtoResponseModel;
export type GetApiV1ProductQueryUploadHistoryDetailsByProductExcelUploadIdApiArg =
  {
    productExcelUploadId: number;
    "api-version"?: string;
    /** DynamicQuery */
    dqb?: string;
  };
export type GetApiV1ProductQueryApiResponse =
  /** status 200 Success */ QueryProductDtodqbResultDtoResponseModel;
export type GetApiV1ProductQueryApiArg = {
  "api-version"?: string;
  /** DynamicQuery */
  dqb?: string;
};
export type GetApiV1ProductGetByProductIdApiResponse =
  /** status 200 Success */ ProductDetailsDtoResponseModel;
export type GetApiV1ProductGetByProductIdApiArg = {
  productId: number;
  "api-version"?: string;
};
export type PostApiV1ProductByProductIdAddImagesApiResponse =
  /** status 200 Success */ ProductImageDto[];
export type PostApiV1ProductByProductIdAddImagesApiArg = {
  productId: number;
  "api-version"?: string;
  body: {
    files?: Blob[];
  };
};
export type GetApiV1TenantGetApiResponse =
  /** status 200 Success */ TenantDtoResponseModel;
export type GetApiV1TenantGetApiArg = {
  "api-version"?: string;
};
export type PostApiV1TenantUpsertApiResponse =
  /** status 200 Success */ TenantDtoResponseModel;
export type PostApiV1TenantUpsertApiArg = {
  "api-version"?: string;
  upsertTenant: UpsertTenant;
};
export type AccountDto = {
  id?: string;
};
export type AccountDtoResponseModel = {
  statusCode?: number;
  data?: AccountDto;
  message?: string | null;
  errorType?: string | null;
  errors?: {
    [key: string]: string[];
  } | null;
};
export type CreateAccount = {
  id?: string;
  email?: string;
};
export type City = {
  name?: string;
  code?: string;
};
export type District = {
  name?: string;
  code?: string;
  cityName?: string;
  cityCode?: string;
};
export type SeedServiceType =
  | "Tenants"
  | "Accounts"
  | "ProductExcelUpload"
  | "Brands"
  | "Products";
export type ProductExcelUploadDto = {
  id?: number;
  tenantId?: number;
  blobId?: string;
  originalFileName?: string;
  extension?: string;
  url?: string;
  contentType?: string;
  fileSizeInBytes?: number;
  fileSize?: string;
  createdAt?: string;
};
export type ProductExcelUploadDtodqbResultDto = {
  data?: ProductExcelUploadDto[];
  count?: number;
};
export type ProductExcelUploadDtodqbResultDtoResponseModel = {
  statusCode?: number;
  data?: ProductExcelUploadDtodqbResultDto;
  message?: string | null;
  errorType?: string | null;
  errors?: {
    [key: string]: string[];
  } | null;
};
export type ProductPropertyName =
  | "None"
  | "Name"
  | "SKU"
  | "Barcode"
  | "Brand"
  | "Category"
  | "Price"
  | "SalesPrice"
  | "StockAmount";
export type ProductErrorMappingsDto = {
  propertyName?: ProductPropertyName;
  errors?: string[];
};
export type QueryProductExcelRowDto = {
  id?: number;
  rowId?: number;
  name?: string;
  sku?: string;
  barcode?: string;
  brand?: string;
  category?: string;
  price?: string;
  salesPrice?: string;
  stockAmount?: string;
  createdAt?: string;
  errorMappings?: ProductErrorMappingsDto[];
};
export type QueryProductExcelRowDtodqbResultDto = {
  data?: QueryProductExcelRowDto[];
  count?: number;
};
export type QueryProductExcelRowDtodqbResultDtoResponseModel = {
  statusCode?: number;
  data?: QueryProductExcelRowDtodqbResultDto;
  message?: string | null;
  errorType?: string | null;
  errors?: {
    [key: string]: string[];
  } | null;
};
export type ProductState =
  | "None"
  | "InReview"
  | "Available"
  | "OutOfStock"
  | "MissingInfo"
  | "Rejected"
  | "Passive";
export type QueryProductDto = {
  id?: number;
  name?: string;
  sku?: string;
  barcode?: string;
  brandId?: number;
  brand?: string;
  category?: string;
  price?: number;
  salesPrice?: number;
  taxRatio?: number;
  state?: ProductState;
  createdAt?: string;
};
export type QueryProductDtodqbResultDto = {
  data?: QueryProductDto[];
  count?: number;
};
export type QueryProductDtodqbResultDtoResponseModel = {
  statusCode?: number;
  data?: QueryProductDtodqbResultDto;
  message?: string | null;
  errorType?: string | null;
  errors?: {
    [key: string]: string[];
  } | null;
};
export type ProductDetailsDto = {
  id?: number;
  name?: string;
  sku?: string;
  barcode?: string;
  brandId?: number;
  brand?: string;
  category?: string;
  price?: number;
  salesPrice?: number;
  taxRatio?: number;
  state?: ProductState;
  createdAt?: string;
};
export type ProductDetailsDtoResponseModel = {
  statusCode?: number;
  data?: ProductDetailsDto;
  message?: string | null;
  errorType?: string | null;
  errors?: {
    [key: string]: string[];
  } | null;
};
export type ProductImageDto = {
  productId?: number;
  name?: string;
  contentType?: string;
  extension?: string;
  url?: string;
};
export type TenantType =
  | "None"
  | "Individual"
  | "LimitedLiability"
  | "IncorporatedCompany";
export type TenantDto = {
  name?: string;
  tckn?: string;
  taxNumber?: string;
  taxOffice?: string;
  phone?: string;
  email?: string;
  city?: string;
  district?: string;
  type?: TenantType;
  isVerified?: boolean;
};
export type TenantDtoResponseModel = {
  statusCode?: number;
  data?: TenantDto;
  message?: string | null;
  errorType?: string | null;
  errors?: {
    [key: string]: string[];
  } | null;
};
export type UpsertTenant = {
  name?: string;
  tckn?: string;
  taxNumber?: string;
  taxOffice?: string;
  phone?: string;
  email?: string;
  city?: string;
  district?: string;
  type?: TenantType;
};
export const {
  useGetApiV1AccountGetQuery,
  useGetApiV1AccountVerifyByEmailQuery,
  usePostApiV1AccountCreateMutation,
  usePostApiV1AccountActivateMutation,
  useGetApiV1AddressGetCitiesQuery,
  useGetApiV1AddressGetDistrictsByCityCodeQuery,
  usePostDevV1SeedMutation,
  useGetDevV1GetClientIpQuery,
  usePostApiV1ProductUploadProductExcelMutation,
  useGetApiV1ProductQueryUploadHistoryQuery,
  useGetApiV1ProductQueryUploadHistoryDetailsByProductExcelUploadIdQuery,
  useGetApiV1ProductQueryQuery,
  useGetApiV1ProductGetByProductIdQuery,
  usePostApiV1ProductByProductIdAddImagesMutation,
  useGetApiV1TenantGetQuery,
  usePostApiV1TenantUpsertMutation,
} = injectedRtkApi;
