import { memo, useEffect, useState } from "react";
import Transition from "./Transition";
import { createPortal } from "react-dom";
import { buildClassName } from "../utils/common";
import { XIcon } from "lucide-react";

function Modal({
	open,
	onClose,
	title,
	className,
	fullscreen = false,
	children,
}: {
	open: boolean;
	onClose: () => void;
	title?: string;
	className?: string;
	fullscreen?: boolean;
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
				<div
					className={buildClassName(
						"Modal",
						className,
						fullscreen && "fullscreen",
					)}
				>
					<div className="Header">
						{title && <div className="title">{title}</div>}
						{fullscreen && (
							<div className="close icon" onClick={onClose}>
								<XIcon />
							</div>
						)}
					</div>
					<div className="Content">{children}</div>
				</div>
			</Transition>,
			document.body,
		)
	);
}

export default memo(Modal);
