import { themes as prismThemes } from 'prism-react-renderer';

export default {
  title: 'FluentHttpClient',
  tagline: 'Use HttpClient with readable and chainable methods.',
  url: 'https://scottoffen.github.io',
  baseUrl: '/fluenthttpclient/',
  onBrokenLinks: 'warn',
  onBrokenMarkdownLinks: 'warn',
  favicon: 'img/favicon.ico',
  trailingSlash: false,

  organizationName: 'scottoffen', // GitHub username
  projectName: 'fluenthttpclient', // Repo name

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  // This enables compatibility with Docusaurus v4 (future-proof)
  future: {
    v4: true,
  },

  presets: [
    [
      'classic',
      {
        docs: {
          routeBasePath: '/', // Serve docs at the root (no /docs prefix)
          sidebarPath: './sidebars.js',
          editUrl: 'https://github.com/scottoffen/fluenthttpclient/edit/main/docs/',
        },
        blog: false, // Disable blog
        theme: {
          customCss: './src/css/custom.css',
        },
      },
    ],
  ],

  themeConfig: {
    navbar: {
      title: 'FluentHttpClient',
      logo: {
        alt: 'FluentHttpClient Logo',
        src: 'img/logo.svg',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'docsSidebar',
          position: 'left',
          label: 'Docs',
        },
        {
          href: 'https://github.com/scottoffen/fluenthttpclient',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Documentation',
          items: [
            {
              label: 'Getting Started',
              to: '/',
            },
          ],
        },
        {
          title: 'Community',
          items: [
            {
              label: 'Discussions',
              href: 'https://github.com/scottoffen/fluenthttpclient/discussions',
            },
            {
              label: 'Stack Overflow',
              href: 'https://stackoverflow.com/questions/tagged/fluenthttpclient',
            },
          ],
        },
        {
          title: 'Project',
          items: [
            {
              label: 'Contributing Guide',
              href: 'https://github.com/scottoffen/fluenthttpclient/blob/main/CONTRIBUTING.md',
            },
            {
              label: 'Code of Conduct',
              href: 'https://github.com/scottoffen/fluenthttpclient/blob/main/CODE_OF_CONDUCT.md',
            },
            {
              label: 'GitHub',
              href: 'https://github.com/scottoffen/fluenthttpclient',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Scott Offen`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
    },
    titleDelimiter: '|',
    titleTemplate: 'FluentHttpClient | %s'
  },
};
