/**
 * @file ftp-client.c
 * @brief A quick ftp upload function with libcurl
 * @author http://curl.haxx.se/lxr/source/docs/examples/ftpupload.c
 *
 * Compile With:
 * gcc -lcurl -DUNIT_TEST -o client ftp-client.c (unit test)
 * gcc -lcurl ftp-client.c (object)
 */

/* system includes */
#include "common.h"
#include <curl/curl.h>
#include <pthread.h>

/* local includes */
#include "gui-logger.h"
#include "ftp-client.h"

/*
 * NOTE: if you want this example to work on Windows with libcurl as a
 * DLL, you MUST also provide a read callback with CURLOPT_READFUNCTION.
 * Failing to do so will give you a crash since a DLL may not use the
 * variable's memory when passed in to it from an app like this.
 */
static size_t read_callback(void *ptr, size_t size, size_t nmemb, void *stream)
{
	/*
	 * in real-world cases, this would probably get this data differently
	 * as this fread() stuff is exactly what the library already would do
	 * by default internally
	 */
	size_t retcode = fread(ptr, size, nmemb, stream);
	
	dprintf("*** We read %d bytes from file\n", retcode);
	return retcode;
}

/**
 * @brief Uploads the specified file with the passed in settings
 * @param input Settings to control the upload
 * @return void
 *
 * @code
 *  struct ftp_upload *settings;
 *  pthread_t thread;
 *  
 *  settings = (struct ftp_upload *)xmalloc(sizeof(struct ftp_upload));
 *  strcpy(settings->host, "172.16.5.22/public");
 *  strcpy(settings->user, "anonymous");
 *  strcpy(settings->pass, "anonymous");
 *  strcpy(settings->file, "test.txt");
 *  pthread_create(&thread, NULL, ftp_upload, (void *)settings);
 *  pthread_exit(NULL);
 * @endcode
 */
void *ftp_upload(void *input)
{
	CURL *curl;
	CURLcode result;
	FILE *hd_src = NULL;
	struct stat file_info;
	struct curl_slist *headerlist=NULL;
	char buffer[250] = "\0";
	char remote[250] = "\0";
	char error[250]  = "\0";
	char *split;
	struct ftp_upload *settings = (struct ftp_upload *)input;

	/* get the filename to put */
	split = strrchr(settings->file, '/');
	sprintf(buffer, "RNFR %s", ++split);
	sprintf(remote, "ftp://%s:%s@%s/%s",
		   	settings->user, settings->pass,
			settings->host, split);

	/* get the file size of the local file */
	if (stat(settings->file, &file_info)) {
		sprintf(error, _("File [%s] Does Not Exist\n"), settings->file);
		gui_error_log(error);
		goto ftp_cleanup;
	}
	hd_src = fopen(settings->file, "rb");	/* get a FILE * of the same file */
	curl_global_init(CURL_GLOBAL_ALL);		/* In windows, this will init the winsock stuff */
	curl = curl_easy_init();				/* get a curl handle */

	if (curl) {

		/* show current status */
		sprintf(error, _("Sending Firmware [%d bytes]..."), (int)file_info.st_size);
		gui_update_log(error);

		/* build a list of commands to pass to libcurl */
		headerlist = curl_slist_append(headerlist, buffer);
		
		/* we want to use our own read function */
		curl_easy_setopt(curl, CURLOPT_READFUNCTION, read_callback);
		
		/* enable uploading */
		curl_easy_setopt(curl, CURLOPT_UPLOAD, 1L);
		
		/* specify target */
		curl_easy_setopt(curl,CURLOPT_URL, remote);
		
		/* pass in that last of FTP commands to run after the transfer */
		curl_easy_setopt(curl, CURLOPT_POSTQUOTE, headerlist);
		
		/* now specify which file to upload */
		curl_easy_setopt(curl, CURLOPT_READDATA, hd_src);
		
		/* Set the size of the file to upload (optional).  If you give a *_LARGE
		   option you MUST make sure that the type of the passed-in argument is a
		   curl_off_t. If you use CURLOPT_INFILESIZE (without _LARGE) you must
		   make sure that to pass in a type 'long' argument. */
		curl_easy_setopt(curl, CURLOPT_INFILESIZE_LARGE,
                     (curl_off_t)file_info.st_size);

		result = curl_easy_perform(curl);		/* Now run off and do what you've been told! */
		curl_slist_free_all(headerlist);	/* clean up the FTP commands list */
		curl_easy_cleanup(curl);			/* always cleanup */

		/* Process result message */
		if (result ==  CURLE_COULDNT_RESOLVE_HOST) {
			sprintf(error, _("Host [%s] Does Not Exist\n"), settings->host);
			gui_error_log(error);
		}
		else if (result == CURLE_OK) {
			gui_update_log(_("Upgrade Successful!\n"));
		}
		else {
			gui_update_log(_("Upgrade Failed!\n"));
		}

	}
ftp_cleanup:
	if (hd_src)
		fclose(hd_src);						/* close the local file */
	free(input);
	curl_global_cleanup();
#ifndef UNIT_TEST
	extern int stop_progress_bar;
	stop_progress_bar = 1;
#endif
	pthread_exit(NULL);
}

#ifdef UNIT_TEST

int main(void)
{
	struct ftp_upload *settings;
	pthread_t thread;

	settings = (struct ftp_upload *)xmalloc(sizeof(struct ftp_upload));
	strcpy(settings->host, "172.16.5.22/public");
	strcpy(settings->user, "anonymous");
	strcpy(settings->pass, "anonymous");
	strcpy(settings->file, "test.txt");
	pthread_create(&thread, NULL, ftp_upload, (void *)settings);
	pthread_exit(NULL);
}

#endif
