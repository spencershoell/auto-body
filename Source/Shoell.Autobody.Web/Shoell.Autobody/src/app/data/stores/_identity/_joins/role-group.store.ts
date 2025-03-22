import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { plainToInstance } from "class-transformer";
import Guid from "devextreme/core/guid";
import { Common, DataFieldTypes, EntityEndpoint } from "src/app/classes";
import { Role_Group, Role_GroupODataStoreModel } from "src/app/data/models";
import { DataEventService } from "src/app/shared/services";
import { environment } from "src/environments/environment";
import ODataStore from "devextreme/data/odata/store";
import { catchError, lastValueFrom, of } from "rxjs";

const url: string = `${environment.settings.resources.api.endpoint}${EntityEndpoint.Role_Group}`;

@Injectable({
    providedIn: 'root'
})
export class Role_GroupStore extends ODataStore<Role_Group, [Guid, Guid]> {

    constructor(
        protected dataEventService: DataEventService,
        protected http: HttpClient
    ) {
        let config = {
            url: url,
            errorHandler: (error: any) => {
                console.error(error);
            },
            version: 4,
            key: ['roleId', 'groupId'],
            keyType: {
                roleId: DataFieldTypes.Guid,
                groupId: DataFieldTypes.Guid
            },
            fieldTypes: Role_GroupODataStoreModel.getFieldTypes(),
            onLoaded: (data: any[]) => {
                for (let i = 0; i < data.length; i++) {
                    data[i] = plainToInstance(Role_Group, data[i]);
                }
            },
            onModified: () => {
                dataEventService.informRoleChanged();
            },
            onUpdating: (key: any, values: any) => {
                delete values.role;
                delete values.group;
            }
        };
        super(config);
    }

    async addToRoles(groupId: Guid, roleIds: Guid[]): Promise<void> {
        await lastValueFrom(this.http
            .post(`${url}/AddToRoles`, { model: { groupId: groupId, roleIds: roleIds } }, {})
            .pipe(
                catchError(error => {
                    if (error instanceof HttpErrorResponse) {
                        Common.toastError(Common.getServerErrorMessage(error.error));
                    } else {
                        Common.toastError(Common.getServerErrorMessage(error.message));
                    }
                    return of();
                })
            ));
    }

    async addToGroups(roleId: Guid, groupIds: Guid[]): Promise<void> {
        await lastValueFrom(this.http
            .post(`${url}/AddToGroups`, { model: { roleId: roleId, groupIds: groupIds } }, {})
            .pipe(
                catchError(error => {
                    if (error instanceof HttpErrorResponse) {
                        Common.toastError(Common.getServerErrorMessage(error.error));
                    } else {
                        Common.toastError(Common.getServerErrorMessage(error.message));
                    }
                    return of();
                })
            ));
    }
}