function handler(event) {
    var request = event.request;
    var uri = request.uri;

    // Check whether the URI is missing a file name.
    if (uri === '/') {
        return request;
    }

    if (uri.endsWith('/')) {
        request.uri = uri.substring(0, uri.length - 1) + '.html';
    }
    // Check whether the URI is missing a file extension.
    else if (!uri.includes('.')) {
        request.uri += '.html';
    }

    return request;
}