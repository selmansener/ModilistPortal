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
  }),
  overrideExisting: false,
});
export { injectedRtkApi as api };
export type GetApiV1AccountGetApiResponse =
  /** status 200 Success */ AccountDto;
export type GetApiV1AccountGetApiArg = {
  "api-version"?: string;
};
export type GetApiV1AccountVerifyByEmailApiResponse = unknown;
export type GetApiV1AccountVerifyByEmailApiArg = {
  email: string;
  "api-version"?: string;
};
export type PostApiV1AccountCreateApiResponse =
  /** status 200 Success */ AccountDto;
export type PostApiV1AccountCreateApiArg = {
  "api-version"?: string;
  createAccount: CreateAccount;
};
export type PostApiV1AccountActivateApiResponse =
  /** status 200 Success */ AccountDto;
export type PostApiV1AccountActivateApiArg = {
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
export type AccountDto = {
  id?: string;
};
export type CreateAccount = {
  id?: string;
  email?: string;
};
export type SeedServiceType = "Accounts";
export const {
  useGetApiV1AccountGetQuery,
  useGetApiV1AccountVerifyByEmailQuery,
  usePostApiV1AccountCreateMutation,
  usePostApiV1AccountActivateMutation,
  usePostDevV1SeedMutation,
  useGetDevV1GetClientIpQuery,
} = injectedRtkApi;
