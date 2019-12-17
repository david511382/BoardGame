import { HttpErrorResponse } from "@angular/common/http";
import { throwError } from "rxjs";

export interface SuccessResponse extends GeneralResponse {
  isSuccess: string
}

export interface GeneralResponse {
  message: string,
  errorMessage: string,
  isError: boolean,
}

export function IsGeneralResponse(obj: any): obj is GeneralResponse {
  if (!obj || !obj.isError || !obj.errorMessage)
    return false;
  return true;
}

export function HandleErrorFun(errorMsgFun: (error: HttpErrorResponse) => string = null) {
  if (errorMsgFun === null) {
    errorMsgFun = (error: HttpErrorResponse) => {
      let errorMsg;
      let response: GeneralResponse = error.error;
      if (IsGeneralResponse(error.error)) {
        errorMsg = response.errorMessage;
      }
      else if (error.error instanceof ErrorEvent) {
        // A client-side or network error occurred. Handle it accordingly.
        //alert(`An error occurred:${error.error.message}`);
        errorMsg = error.error.message;
      }
      else {
        // The backend returned an unsuccessful response code.
        // The response body may contain clues as to what went wrong,
        //alert(`Backend returned code ${error.status}, ` +`body was: ${error.error}`);
        errorMsg = JSON.stringify(error);
      }

      return `Something bad happened; please try again later.\n${error.status}\n${errorMsg}`;
      
    }
  }

  return (error: HttpErrorResponse) => {
    var response: GeneralResponse = error.error;
    if (IsGeneralResponse(error.error)) {}
    else if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      //alert(`An error occurred:${error.error.message}`);
      response.errorMessage = error.error.message;
    }
    else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      //alert(`Backend returned code ${error.status}, ` +`body was: ${error.error}`);
      response.errorMessage = "未知的錯誤";
    }

    alert(response.errorMessage);

    // return an observable with a user-facing error message
    var errorMsg = errorMsgFun(error);      
    return throwError(errorMsg);
  };
};
