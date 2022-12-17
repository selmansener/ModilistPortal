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
export type SeedServiceType = "Accounts";
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
  phone?: number;
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
  phone?: number;
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
  useGetApiV1TenantGetQuery,
  usePostApiV1TenantUpsertMutation,
} = injectedRtkApi;
