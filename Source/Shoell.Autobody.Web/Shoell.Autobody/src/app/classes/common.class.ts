import Guid from "devextreme/core/guid";
import notify from "devextreme/ui/notify";

export class Common {
    public static isNull(value: any): boolean {
        if (value !== undefined && value !== null) {
            if (Array.isArray(value)) {
                if (value.length < 1) {
                    return true;
                }
            }
            return false;
        }
        return true;
    }

    public static isNotNull(value: any): boolean {
        return !Common.isNull(value);
    }

    public static phoneMaskRules = { "X": /[02-9]/ };

    public static find(value: any, array: any[], property: string = ""): any {
        let result = null;
        if (this.isNotNull(value) && this.isNotNull(array)) {
            array.some((element: any): boolean => {
                if (Common.isNotNull(property)) {
                    if (value == element[property]) {
                        result = element;
                        return true;
                    }
                    let v: Guid = value;
                    let p: Guid = element[property];
                    if (v.valueOf() === p.valueOf()) {
                        result = element;
                        return true;
                    }
                } else {
                    if (value === element) {
                        result = element;
                        return true;
                    }
                }
                return false;
            });
        }
        return result;
    }

    public static getServerErrorMessage(error: any): string {
        let result = "";
        if (Common.isNotNull(error) && Common.isNotNull(error.value)) {
            let firstElement = true;
            error.value.forEach((element: string) => {
                if (!firstElement) {
                    result += " ";
                }

                result += element;

                if (firstElement) {
                    firstElement = false;
                }
            });
        }
        return result;
    }

    public static toastError(message: string): void {
        notify({
            message: message,
            type: 'error',
            displayTime: 7000
        }, {
            position: 'bottom right',
            direction: 'up-push'
        });
    }

    public static toastSuccess(message: string): void {
        notify({
            message: message,
            type: 'success',
            displayTime: 3000
        }, {
            position: 'bottom right',
            direction: 'up-push'
        });
    }
}