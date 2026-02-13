import { memo, useEffect, useState } from "react";
import Transition from "./Transition";
import { createPortal } from "react-dom";

function Modal({
	open,
	onClose,
	title,
	children,
}: {
	open: boolean;
	onClose: () => void;
	title?: string;
	children: React.ReactNode;
}) {
	const [portal, setPortal] = useState(false);

	useEffect(() => {
		if (open) setPortal(true);
	}, [open]);

	return (
		portal &&
		createPortal(
			<Transition state={open} eachElement action={() => setPortal(false)}>
				<div className="bg" onClick={onClose}></div>
				<div className="Modal">
					<div className="Header">
						{title && <div className="title">{title}</div>}
					</div>
					<div className="Content">{children}</div>
				</div>
			</Transition>,
			document.body,
		)
	);
}

export default memo(Modal);
