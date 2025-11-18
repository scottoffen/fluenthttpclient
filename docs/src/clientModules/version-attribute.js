// This is a custom module for adding a data-doc-version attribute to
// the html tag, primarily to be able to adjust styles per version.

import ExecutionEnvironment from '@docusaurus/ExecutionEnvironment';

function setDocVersionAttribute(pathname) {

  const path = pathname.replace('/fluenthttpclient', '');

  let version = 'current';

  if (path.startsWith('/4.x')) {
    version = '4.x';
  } else if (path.startsWith('/3.x')) {
    version = '3.x';
  }

  document.documentElement.setAttribute('data-doc-version', version);
}

// Run once on initial load
if (ExecutionEnvironment.canUseDOM) {
  setDocVersionAttribute(window.location.pathname);
}

// Run again on every client-side route change
export function onRouteDidUpdate({location}) {
  if (!ExecutionEnvironment.canUseDOM) {
    return;
  }

  setDocVersionAttribute(location.pathname);
}
