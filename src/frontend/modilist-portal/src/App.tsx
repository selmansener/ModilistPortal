import React from 'react';
import logo from './logo.svg';
import './App.css';
import CssBaseline from '@mui/material/CssBaseline';
import { store } from './store/store';
import { AuthenticatedTemplate, MsalProvider, UnauthenticatedTemplate } from "@azure/msal-react";
import { createTheme, ThemeProvider, responsiveFontSizes } from '@mui/material';
import { PublicClientApplication } from '@azure/msal-browser';
import { config } from './config';
import { Provider } from 'react-redux';
import { Router } from './router/router';
import { routes } from './router/routes';
import i18n from "i18next";
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from "react-i18next";
import Backend from 'i18next-http-backend';
import { Authentication } from './store/auth/Authentication';

export const msal = new PublicClientApplication(config.msalConfig);

declare module '@mui/material/styles' {
  interface PaletteColor {
    transparent?: string;
  }

  interface SimplePaletteColorOptions {
    transparent?: string;
  }
}

const mdTheme = createTheme({
  palette: {
    primary: {
      main: '#2F2244',
      contrastText: '#fff'
    },
    secondary: {
      main: '#968DB3',
      transparent: 'rgb(150, 141, 179, 0.2)'
    },
    error: {
      main: '#F08279',
    },
    warning: {
      main: '#FAD27F',
    },
    info: {
      main: '#BEE1E2',
    },
    success: {
      main: '#A6D3B2',
    },
    text: {
      primary: '#2F2244',
      secondary: 'rgba(64,62,86,0.7)',
      disabled: 'rgba(64,62,86,0.38)',
    },
  },
  typography: {
    allVariants: {
      color: '#2F2244',
    },
    fontFamily: 'Poppins, sans-serif',
    h1: {
      fontSize: 64,
      fontFamily: 'Poppins, sans-serif',
      fontWeight: 800,
    },
    h2: {
      fontSize: 40,
      fontFamily: 'Poppins, sans-serif',
      fontWeight: 800,
    },
    h3: {
      fontSize: 32,
      fontFamily: 'Poppins, sans-serif',
      fontWeight: 800,
    },
    h4: {
      fontSize: 24,
      fontFamily: 'Poppins, sans-serif',
      fontWeight: 800,
    },
    h5: {
      fontSize: 20,
      fontFamily: 'Poppins, sans-serif',
      fontWeight: 800,
    },
    h6: {
      fontSize: 16,
      fontFamily: 'Poppins, sans-serif',
      fontWeight: 800,
    },
    subtitle1: {
      fontFamily: 'Poppins, sans-serif',
      fontSize: 20,
    },
    subtitle2: {
      fontFamily: 'Poppins, sans-serif',
      fontSize: 14,
    },
    body1: {
      fontFamily: 'Poppins, sans-serif',
      fontSize: 16,
    },
    body2: {
      fontFamily: 'Poppins, sans-serif',
      fontSize: 12,
      fontWeight: 300,
      lineHeight: 1.27,
    },
    fontWeightLight: 300,
    htmlFontSize: 16,
    fontWeightBold: 800,
    button: {
      fontSize: 16,
      fontWeight: 800,
      textTransform: 'initial',
    },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: `
        ::-webkit-scrollbar {
          width: 10px;
        }

        ::-webkit-scrollbar-track {
          box-shadow: inset 0 0 5px #968DB3;
        }

        ::-webkit-scrollbar-thumb {
          background: #2F2244;
        },

        & .MuiLink-root {
          text-decoration: none;
          color: #BCBC86;
        }

        a {
          text-decoration: none;
        }

        // a.active[aria-current='page'] {
        //   & .MuiListItemButton-root {
        //     background-color: #2F2244
        //   }

        //   & .MuiListItemButton-root:active {
        //     background-color: #2F2244
        //   }

        //   & .MuiTypography-root, & .MuiSvgIcon-root {
        //     color: #fff
        //   }

        //   & .MuiButton-text {
        //     text-decoration: underline;
        //   }
        // }
      `
    },
    MuiLink: {
      styleOverrides: {
        root: {
          textDecoration: 'underline',
          color: '#2F2244'
        }
      }
    },
    MuiButton: {
      styleOverrides: {
        contained: {
          border: 1,
          borderColor: '#2F2244',
          borderStyle: 'solid'
        },
        root: {
          "&.Mui-disabled": {
            border: 0
          }
        }
      }
    }
  }
});

const theme = responsiveFontSizes(mdTheme, {
  factor: 4
});

i18n
  .use(LanguageDetector)
  .use(Backend)
  .use(initReactI18next)
  .init({
    backend: {
      loadPath: '/locales/{{lng}}.json'
    },
    detection: {
      order: ['cookie', 'localStorage', 'sessionStorage', 'navigator', 'querystring', 'htmlTag', 'path', 'subdomain']
    },
    fallbackLng: "tr",
    interpolation: {
      escapeValue: false // react already safes from xss => https://www.i18next.com/translation-function/interpolation#unescape
    }
  });

function App() {
  return (
    <div className="App">
      <CssBaseline enableColorScheme />
      <Provider store={store}>
        <MsalProvider instance={msal}>
          <Authentication />
          <ThemeProvider theme={theme} >
            <AuthenticatedTemplate>
              <Router routes={routes} isPublic={false} currentAccountRole="user" environment={config.environment} />
            </AuthenticatedTemplate>
            <UnauthenticatedTemplate>
              <Router routes={routes} isPublic={true} currentAccountRole="user" environment={config.environment} />
            </UnauthenticatedTemplate>
          </ThemeProvider>
        </MsalProvider>
      </Provider>
    </div>
  );
}

export default App;
