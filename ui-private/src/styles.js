import 'normalize.css/normalize.css';
import 'antd/dist/antd.css';
import { createGlobalStyle } from 'styled-components/macro';

export const StyledWhiteTheme = createGlobalStyle`
  @import url('https://fonts.googleapis.com/css?family=Roboto:400,700');

  :root {
    --font-serif: Roboto, 'Segoe UI', 'Ubuntu', 'Helvetica Neue', sans-serif;
  }
`;

export const StyledLayout = createGlobalStyle`
  body {
    font-family: var(--font-serif);
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
  }

  #root {
    height: 100%;
  }
`;
