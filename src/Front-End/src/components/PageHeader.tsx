import { Children, memo, useEffect, type ReactNode } from "react";
import useUIStore from "../stores/useUIStore";

function PageHeader({ children }: { children: ReactNode }) {
	return children;
}

export function PageHeaderTitle({ children }: { children: ReactNode }) {
	const { setTopBarTitle } = useUIStore();

	useEffect(() => {
		const first = Children.toArray(children)[0];

		if (typeof first === "string") {
			setTopBarTitle(first);
		}
	}, [children]);

	return <></>;
}

export default memo(PageHeader);
