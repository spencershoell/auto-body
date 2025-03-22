import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { GroupListComponent } from "src/app/shared/components/_identity/group/list.component";

@Component({
    selector: 'sh-group-list-page',
    templateUrl: 'list.page.html',
    styleUrls: ['list.page.scss'],
    imports: [RouterModule, GroupListComponent]
})
export class GroupListPageComponent { }