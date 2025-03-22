import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { UserListComponent } from "src/app/shared/components/_identity/user/list.component";

@Component({
    selector: 'sh-user-list-page',
    templateUrl: 'list.page.html',
    styleUrls: ['list.page.scss'],
    imports: [RouterModule, UserListComponent]
})
export class UserListPageComponent { }