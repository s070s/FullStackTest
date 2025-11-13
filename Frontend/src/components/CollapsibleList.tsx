import React from "react";
import Button from "./Button";



/**
 * CollapsibleList Component
 * For displaying a list of items that can be expanded or collapsed.
 */

interface CollapsibleListProps {
    title: string;
    items: number[];
    initiallyExpanded?: boolean;
    className?: string;
}


const CollapsibleList: React.FC<CollapsibleListProps> =({
    title,
    items,
    initiallyExpanded = false,
    className,
}) => {
    const collapsibleListClass = className ? className : "collapsible-list-component";
    const [expanded, setExpanded] = React.useState(initiallyExpanded);
    return (
        <div className={collapsibleListClass}>
            
            {/*Expand-Collapse Button */}
            <Button onClick={() => setExpanded(!expanded)}>
                <i className={expanded ? "fa fa-square-caret-down" : "fa fa-square-caret-right"}/>
                {' '}
                <strong>{title}</strong>
            </Button>
            {
                expanded && 
                (
                    <ul className="collapsible-list">
                        {
                            items && items.length > 0 ?
                            (
                                items.map
                                (
                                    (item, i) => <li key={i}>{item}</li>
                                )
                            ):(<li>No {title} available</li>)
                        }
                    </ul>
                )
            }
        </div>
    );
}

export default CollapsibleList;
